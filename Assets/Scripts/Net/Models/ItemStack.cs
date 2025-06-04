using System;

namespace BiotonicFrontiers.Net.Models
{
    /// <summary>
    /// Net-layer DTO representing an inventory entry returned by
    /// the `/api/inventory` endpoints.  
    /// It mirrors the server’s JSON schema and is kept separate from the
    /// UI-oriented <see cref="BiotonicFrontiers.Data.ItemStack"/> struct.
    /// </summary>
    [Serializable]
    public struct ItemStack
    {
        /// <summary>Identifier of the item (matches server-side <c>item_id</c>).</summary>
        public int item_id;

        /// <summary>Number of items in the stack.</summary>
        public int quantity;
    }
}
