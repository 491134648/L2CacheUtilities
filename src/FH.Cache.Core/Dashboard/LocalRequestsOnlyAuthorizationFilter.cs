﻿// This file is part of FH.Cache.Core.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// FH.Cache.Core is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// FH.Cache.Core is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with FH.Cache.Core. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace FH.Cache.Core.Dashboard
{
    public class LocalRequestsOnlyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // if unknown, assume not local
            if (String.IsNullOrEmpty(context.Request.RemoteIpAddress))
                return false;

            // check if localhost
            if (context.Request.RemoteIpAddress == "127.0.0.1" || context.Request.RemoteIpAddress == "::1")
                return true;

            // compare with local address
            if (context.Request.RemoteIpAddress == context.Request.LocalIpAddress)
                return true;

            return false;
        }
    }
}
