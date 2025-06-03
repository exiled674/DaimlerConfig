using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Services
{
    public class ToolLockService
    {
        private readonly Dictionary<int, string> _locks = new Dictionary<int, string>();

        public bool AcquireLock(int toolId, string userId)
        {
            if (_locks.ContainsKey(toolId))
            {
                return false; // Lock is already held by another user
            }

            _locks[toolId] = userId;
            return true;
        }

        public void ReleaseLock(int toolId)
        {
            if (_locks.ContainsKey(toolId))
            {
                _locks.Remove(toolId);
            }
        }

        public bool IsLocked(int toolId, string userId)
        {
            return _locks.ContainsKey(toolId) && _locks[toolId] != userId;
        }
    }

}
