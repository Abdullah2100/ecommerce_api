namespace api.shared.mapper;

public static class OrderStatusMapperExtension
{
    extension(int orderStatus)
    {
        public string ToOrderStatusName()
        {
            return orderStatus switch
            {
                0 => "Rejected",
                1 => "Inprogress",
                2 => "Accepted",
                3 => "In away",
                4 => "Received",
                _ => "Completed",
            };
        }
    }
}