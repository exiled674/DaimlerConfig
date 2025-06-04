using System;
using System.Threading.Tasks;

namespace DaimlerConfig.Services
{
    public class DirtyManagerService
    {
        private bool isDirty = false;

        private async Task setDirty()
        {
            if (isDirty) return;
            isDirty = true;
        }

        public async Task setClean()
        {
            if (!isDirty) return;
            isDirty = false;
        }

        public async Task<bool> IsDirty()
        {
            return isDirty;
        }

        
        public async Task<bool> CheckIfDirty<T>(T original, T current) where T : IEquatable<T>
        {
            
            bool result = !(original?.Equals(current) ?? current == null);
            if (result)
                await setDirty();
            else
                await setClean();
            return result;
        }


    }
}
