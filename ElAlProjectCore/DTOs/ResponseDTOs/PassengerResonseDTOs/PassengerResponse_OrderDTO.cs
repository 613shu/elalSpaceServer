namespace ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs
{
    public class PassengerResponse_OrderDTO
    {
        public int Id { get; set; }
        public PassengerResponse_FlightDTO Flight { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; }
    }
}
