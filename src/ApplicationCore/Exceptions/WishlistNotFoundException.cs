using System;

namespace ApplicationCore.Exceptions
{
    public class WishlistNotFoundException: Exception
    {
       public WishlistNotFoundException(int wishlistId) : base($"No wishlist found with id {wishlistId}")
        {
        }

        protected WishlistNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public WishlistNotFoundException(string message) : base(message)
        {
        }

        public WishlistNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
