﻿// This file is part of Hangfire.
// Copyright © 2016 Sergey Odinokov.
// 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FH.Cache.Core.Dashboard;
using Microsoft.AspNetCore.Http;

namespace Hangfire.Dashboard
{
    internal sealed class AspNetCoreDashboardRequest : DashboardRequest
    {
        private readonly HttpContext _context;

        public AspNetCoreDashboardRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public override string Method => _context.Request.Method;
        public override string Path => _context.Request.Path.Value;
        public override string PathBase => _context.Request.PathBase.Value;
        public override string LocalIpAddress => _context.Connection.LocalIpAddress.ToString();
        public override string RemoteIpAddress => _context.Connection.RemoteIpAddress.ToString();
        public override string GetQuery(string key) => _context.Request.Query[key];

        public override async Task<IList<string>> GetFormValuesAsync(string key)
        {
            var form = await _context.Request.ReadFormAsync();
            return form[key];
        }
    }
}
