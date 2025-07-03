using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfig.Services
{
    public class AppLifecycleService
    {
        public event Func<Task>? OnAppClosing;

        public void RaiseAppClosingSync()
        {
            if (OnAppClosing != null)
            {
                var done = new ManualResetEventSlim(false);

                _ = Task.Run(async () =>
                {
                    await OnAppClosing.Invoke();
                    done.Set();
                });

                done.Wait(); // Blockiert App-Schluss bis Unlock + SignalR fertig ist
            }
        }

    }
}