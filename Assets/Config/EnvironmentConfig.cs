public static class EnvironmentConfig
{
    public static readonly string NodeUrl   =
        System.Environment.GetEnvironmentVariable("APTOS_NODE_URL") ??
        "https://fullnode.devnet.aptoslabs.com";

    public static readonly string FaucetUrl =
        System.Environment.GetEnvironmentVariable("APTOS_FAUCET_URL") ??
        "https://faucet.devnet.aptoslabs.com";
}
