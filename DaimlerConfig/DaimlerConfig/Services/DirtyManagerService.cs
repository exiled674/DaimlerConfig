using System;
using System.Threading.Tasks;

namespace DConfig.Services
{
    public class DirtyManagerService
    {
        private bool isDirty = false;
        public string dirtyEntity = "";

        private async Task setDirty(string entity)
        {
            if (isDirty) return;
            isDirty = true;
            dirtyEntity = entity;
        }

        public async Task setClean()
        {
            if (!isDirty) return;
            isDirty = false;
            dirtyEntity = "";
        }

        public async Task<bool> IsDirty()
        {
            return isDirty;
        }

        
        public async Task<bool> CheckIfDirty<T>(T original, T current, string entity) where T : IEquatable<T>
        {
            
            bool result = !(original?.Equals(current) ?? current == null);
            if (result)
                await setDirty(entity);
            else
                await setClean();
            return result;
        }
        

    }
}
