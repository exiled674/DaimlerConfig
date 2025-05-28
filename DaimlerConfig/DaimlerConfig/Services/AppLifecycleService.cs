using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Services
{
    public class AppLifecycleService
    {
        public event Func<Task>? OnAppClosing;

        public async Task RaiseAppClosingAsync()
        {
            if (OnAppClosing != null)
                await OnAppClosing.Invoke();
        }
    }
}