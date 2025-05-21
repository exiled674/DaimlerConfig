using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Services
{
    public class DirtyManagerService
    {
        private Boolean isDirty = false;

        private async Task setDirty()
        {
            if (isDirty == true) return;
            isDirty = true;
        }

        private async Task setClean()
        {
            if (isDirty == false) return;
            isDirty = false;
        }

        public async Task<bool> IsDirty()
        {
            return isDirty;
        }
    }
}
