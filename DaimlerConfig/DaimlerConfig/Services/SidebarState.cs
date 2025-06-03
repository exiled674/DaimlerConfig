using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Services
{
    public class SidebarState
    {
        public event Action? OnToggleRequested;

        public void RequestToggle()
        {
            OnToggleRequested?.Invoke();
        }
    }

}
