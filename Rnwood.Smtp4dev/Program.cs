﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Rnwood.Smtp4dev.Service;

namespace Rnwood.Smtp4dev
{
    public class Program
    {
        public static bool IsService { get; private set; }

        public static void Main(string[] args)
        {
            if (!Debugger.IsAttached && args.Contains("--service"))
                IsService = true;

            var host = CreateWebHost(args.Where(arg => arg != "--service").ToArray());

            if (IsService)
            {
                host.RunAsSmtp4devService();
            }
            else
            {
                host.Run();
            }
        }

        private static string GetContentRoot()
        {
            string installLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            if (Directory.Exists(Path.Join(installLocation, "wwwroot")))
            {
                return installLocation;
            }

            string cwd = Directory.GetCurrentDirectory();
            if (Directory.Exists(Path.Join(cwd, "wwwroot")))
            {
                return cwd;
            }

            throw new ApplicationException($"Unable to find wwwroot in either '{installLocation}' or the CWD '{cwd}'");
        }

        private static IWebHost CreateWebHost(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .UseContentRoot(GetContentRoot())
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                        {
                            var env = hostingContext.HostingEnvironment;
                            config
                                .SetBasePath(env.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .AddCommandLine(args, new
                                Dictionary<string, string>{
                                    { "--smtpport", "ServerOptions:Port"}
                                })
                                .Build();
                        })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
