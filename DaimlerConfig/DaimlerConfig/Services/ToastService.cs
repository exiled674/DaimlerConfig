using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Services
{
    public class ToastService
    {
        public event Action<string>? OnShowSuccess;
        public event Action<string>? OnShowError;

        public void ShowSuccess(string message)
        {
            OnShowSuccess?.Invoke(message);
        }

        public void ShowError(string message)
        {
            OnShowError?.Invoke(message);
        }
    }
}
