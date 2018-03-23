using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NHttp;
using System.Net;
using System.Windows.Forms;
using System.Drawing;

namespace ZplWeb2Usb 
{
     class Program : System.Windows.Forms.Form
     {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem;
        private System.ComponentModel.IContainer components;
        private HttpServer server;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            using (var server = new HttpServer())
            {
                // New requests are signaled through the RequestReceived
                // event.

                server.RequestReceived += (s, e) =>
                {
                    // The response must be written to e.Response.OutputStream.
                    // When writing text, a StreamWriter can be used.

                    using (var writer = new StreamWriter(e.Response.OutputStream))
                    {
                        writer.WriteLine("SERVICIO DE IMPRESION DE ETIQUETAS DE LABORATORIOS MARRUJO INICIADO");
                        
                        String validApi = "4338A30228604B77A67FC52C313F01A0";

                        if (e.Request.QueryString.Get("apiKey") != validApi) {
                            writer.WriteLine("E / API KEY: " + e.Request.QueryString.Get("apiKey"));
                            return;
                        }
                        String data;
                        byte[] bytes;

                        switch (e.Request.Path.TrimEnd('/')) {
                            case "/print":
                                data = e.Request.QueryString.Get("data");
                                bytes = Convert.FromBase64String(data);
                                RawPrinterHelper.SendStringToPrinter(e.Request.QueryString.Get("printer"), bytes, bytes.Length);
                                break;
                            case "/test":
                                data = "XlhBDQoNCl5CWTUsMiwyNzANCl5GTzE3NSw1NTBeQkNeRkQxMjM0NTY3ODkwXkZTDQoNCl5DRjAsNDANCl5GTzEwMCw5NjBeRkRFVElRVUVUQSBERSBQUlVFQkENCg0KXlha";
                                bytes = Convert.FromBase64String(data);
                                RawPrinterHelper.SendStringToPrinter(e.Request.QueryString.Get("printer"), bytes, bytes.Length);
                                break;
                            default:
                                writer.WriteLine("INVALID OPERATION "+e.Request.RawUrl+" "+e.Request.Url+" "+e.Request.Path);
                                break;
                        }
                    }
                };

                // Start the server on a random port. Use server.EndPoint
                // to specify a specific port, e.g.:
                //
                //     server.EndPoint = new IPEndPoint(IPAddress.Loopback, 80);
                //
                server.EndPoint = new IPEndPoint(IPAddress.Loopback, 65101);
                server.Start();

                // Start the default web browser.
                
                Application.Run(new Program(server));
                
                // When the HttpServer is disposed, all opened connections
                // are automatically closed.
            }
        }
        public Program(HttpServer server)
        {
            this.server = server;
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.menuItem });

            // Initialize menuItem1
            this.menuItem.Index = 0;
            this.menuItem.Text = "Salir";
            this.menuItem.Click += new System.EventHandler(this.menuItem_Click);

            // Set up how the form should be displayed.
            //this.ClientSize = new System.Drawing.Size(400, 50);
            this.Text = "Servicio de Impresion de etiquetas";

            this.WindowState = FormWindowState.Minimized;
            // Create the NotifyIcon.
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon.Icon = new Icon("appicon.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon.ContextMenu = this.contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon.Text = "El servicio ha sido iniciado";
            notifyIcon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
        }
        protected override void Dispose(bool disposing)
        {
            // Clean up any components being used.
            if (disposing)
                if (components != null)
                    components.Dispose();

            base.Dispose(disposing);
        }

        private void notifyIcon_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            //if (this.WindowState == FormWindowState.Minimized)
            //  this.WindowState = FormWindowState.Normal;

            // Activate the form.
            //this.Activate();
            Process.Start(String.Format("http://{0}/", server.EndPoint));
        }

        private void menuItem_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }


    }

}