namespace Domain.Entities.UKG
{
    public class OAuthRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Client_Id { get; set; }
        public required string Client_Secret { get; set; }
        public required string Grant_Type { get; set; }
        public required string Auth_Chain { get; set; }
    }
}
