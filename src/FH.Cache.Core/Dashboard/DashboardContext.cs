﻿using System;
using System.Text.RegularExpressions;

namespace FH.Cache.Core.Dashboard
{
    public abstract class DashboardContext
    {
        private readonly Lazy<bool> _isReadOnlyLazy;

        protected DashboardContext(DashboardOptions options, IMonitoringApi storage)
        {
          
            if (options == null) throw new ArgumentNullException(nameof(options));
            Options = options;
            _isReadOnlyLazy = new Lazy<bool>(() => options.IsReadOnlyFunc(this));
            Storage = storage;
        }
        public IMonitoringApi Storage { get; }
        public DashboardOptions Options { get; }

        public Match UriMatch { get; set; }
        
        public DashboardRequest Request { get; protected set; }
        public DashboardResponse Response { get; protected set; }

        public bool IsReadOnly => _isReadOnlyLazy.Value;

        public string AntiforgeryHeader { get; set; }
        public string AntiforgeryToken { get; set; }
    }
}