using AmsiWrapper;
using LazyHttpServerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AmsiWebUi
{
    internal class Program
    {
        private static AmsiWrapper.AmsiWrapper amsi;
        private static HttpServer httpd;

        static void Main(string[] args)
        {
            amsi = new AmsiWrapper.AmsiWrapper("AmsiWebUi");
            httpd = new HttpServer(new string[] { "http://127.0.0.1:8080/" });

            httpd.IncomingHttpRequest += Httpd_IncomingHttpRequest;

            httpd.Start();

            Console.WriteLine("Listening http request");
            Console.WriteLine("Press return to exit");
            Console.ReadLine();

            httpd.Stop();
            //amsi.Dispose();
        }

        private static void Httpd_IncomingHttpRequest(object sender, IncomingHttpRequestEventArgs e)
        {
            Console.WriteLine($"=> {e.Request.HttpMethod} {e.Request.Url.AbsolutePath} ({e.Request.ContentLength64} bytes)");

            switch(e.Request.Url.AbsolutePath)
            {
                case "/scan":
                    {
                        byte[] return_bytes;
                        if (e.Request.HttpMethod != "POST")
                        {
                            e.Response.StatusCode = 405;
                            return_bytes = Encoding.UTF8.GetBytes("METHOD_NOT_ALLOWED");
                            e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                            Console.WriteLine($"<= 405: {return_bytes.Length} bytes");
                            break;
                        }

                        byte[] data = new byte[e.Request.ContentLength64];
                        e.Request.InputStream.Read(data, 0, data.Length);

                        var scan_result = amsi.ScanBuffer(data, "http_request.bin");

                        e.Response.StatusCode = 200;
                        e.Response.ContentType = "application/json";
                        return_bytes = Encoding.UTF8.GetBytes("{ \"result\": \"" + scan_result + "\" }");
                        e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                        Console.WriteLine($"<= 200: {return_bytes.Length} bytes");
                        break;
                    }

                case "/scanstr":
                    {
                        byte[] return_bytes;
                        if (e.Request.HttpMethod != "POST")
                        {
                            e.Response.StatusCode = 405;
                            return_bytes = Encoding.UTF8.GetBytes("METHOD_NOT_ALLOWED");
                            e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                            Console.WriteLine($"<= 405: {return_bytes.Length} bytes");
                            break;
                        }

                        byte[] data = new byte[e.Request.ContentLength64];
                        e.Request.InputStream.Read(data, 0, data.Length);
                        string strdata = Encoding.UTF8.GetString(data);

                        var scan_result = amsi.ScanString(strdata, "http_request.txt");

                        e.Response.StatusCode = 200;
                        e.Response.ContentType = "application/json";
                        return_bytes = Encoding.UTF8.GetBytes("{ \"result\": \"" + scan_result + "\" }");
                        e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                        Console.WriteLine($"<= 200: {return_bytes.Length} bytes");
                        break;
                    }

                case "/index.html":
                case "/":
                    {
                        byte[] return_bytes;
                        if (e.Request.HttpMethod != "GET")
                        {
                            e.Response.StatusCode = 405;
                            return_bytes = Encoding.UTF8.GetBytes("METHOD_NOT_ALLOWED");
                            e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                            Console.WriteLine($"<= 405: {return_bytes.Length} bytes");
                            break;
                        }

                        e.Response.StatusCode = 200;
                        e.Response.ContentType = "text/html; charset=utf-8";
                        return_bytes = Encoding.UTF8.GetBytes(@"<!DOCTYPE html>
<html>
<head>
    <title>AMSI Web UI</title>
</head>
<body>
    <h1>AMSI Web UI</h1>
    <form action=""/scan"" method=""post"">
        <h2>File scan</h2>
        <label for=""scanFile"">Scan file</label>
        <input type=""file"" name=""file"" id=""scanFile""/><br />
        <button type=""button"" onclick=""scan()"" id=""scanButton"">Scan</button>
    </form>

    <form action=""/scanstr"" method=""post"">
        <h2>String scan</h2>
        <label for=""scanStrFile"">Scan text</label>
        <textarea name=""str"" id=""scanStrFile""></textarea><br />
        <button type=""button"" onclick=""scanStr()"" id=""scanStrButton"">Scan</button>
    </form>

    <script>
        function disableButton() {
            document.getElementById('scanButton').disabled = true;
            document.getElementById('scanStrButton').disabled = true;
        }

        function enableButton() {
            document.getElementById('scanButton').disabled = false;
            document.getElementById('scanStrButton').disabled = false;
        }

        function scan() {
            disableButton();

            var files = document.getElementById('scanFile').files;
            if (files.length == 0) {
                alert('Please select a file');
                enableButton();
                return;
            }

            fetch('/scan', {
                method: 'POST',
                body: files[0],
                headers: {
                    'Content-Type': 'application/octet-stream'
                }
            }).then(response => response.json()).then(data => {
                alert(data.result);
                enableButton();
            }).catch(() => {
                alert('Error');
                enableButton();
            });
        }

        function scanStr() {
            disableButton();
            var str = document.getElementById('scanStrFile').value;
            if (str.length == 0) {
                alert('Please input text');
                enableButton();
                return;
            }

            fetch('/scanstr', {
                method: 'POST',
                body: str,
                headers: {
                    'Content-Type': 'text/plain'
                }
            }).then(response => response.json()).then(data => {
                alert(data.result);
                enableButton();
            }).catch(() => {
                alert('Error');
                enableButton();
            });
        }
    </script>
</body>
</html>");
                        e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                        Console.WriteLine($"<= 200: {return_bytes.Length} bytes");
                        break;
                    }

                default:
                    {
                        e.Response.StatusCode = 404;
                        e.Response.ContentType = "text/plain";
                        var return_bytes = Encoding.UTF8.GetBytes("Not found");
                        e.Response.OutputStream.Write(return_bytes, 0, return_bytes.Length);
                        Console.WriteLine($"<= {return_bytes.Length} bytes");
                        break;
                    }
            }

            e.Response.Close();
        }
    }
}
