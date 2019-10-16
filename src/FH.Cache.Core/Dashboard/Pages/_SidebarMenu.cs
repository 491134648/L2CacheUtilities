// This file is part of FH.Cache.Core.
// Copyright © 2015 Sergey Odinokov.
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

namespace FH.Cache.Core.Dashboard.Pages
{
    partial class SidebarMenu
    {
        public SidebarMenu(IEnumerable<Func<RazorPage, MenuItem>> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            Items = items;
        }

        public IEnumerable<Func<RazorPage, MenuItem>> Items { get; }
    }
}
