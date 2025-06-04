using System;

namespace BiotonicFrontiers.Data.DTO
{
    [Serializable]
    public struct TradeInfo
    {
        public string trade_id;
        public string creator_id;
        public string offered_item_id;
        public int    offered_quantity;
        public string requested_item_id;
        public int    requested_quantity;
    }

    [Serializable]
    public struct TradeCreateReq
    {
        public string player_id;
        public string offered_item_id;
        public int    offered_quantity;
        public string requested_item_id;
        public int    requested_quantity;
    }

    [Serializable]
    public struct AcceptTradeReq
    {
        public string player_id;
        public string trade_id;
    }
}
