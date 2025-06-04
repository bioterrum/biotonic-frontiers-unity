// -----------------------------------------------------------------------------
//  AptosSdkFallback.cs  – minimal stubs so the project still compiles when the
//  official Aptos Unity SDK is *not* in the project.  Guarded by the same
//  APTOS_SDK_PRESENT define used elsewhere.
// -----------------------------------------------------------------------------
#if !APTOS_SDK_PRESENT
using Cysharp.Threading.Tasks;

namespace Aptos.Unity.Rest
{
    /// <summary>Dummy tx model matching the property used in client code.</summary>
    internal struct Transaction { public bool Success; }

    /// <summary>Lightweight no-op REST client stub.</summary>
    internal sealed class Client
    {
        public Client(string _) { }
        public UniTask<Transaction> GetTransaction(string _)
            => UniTask.FromResult(new Transaction { Success = true });
    }
}
#endif
