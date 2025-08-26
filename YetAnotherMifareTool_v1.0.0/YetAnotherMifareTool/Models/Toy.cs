namespace YetAnotherMifareTool.Models
{
    internal class Toy
    {
        public byte[] ManufacturerBlock { get; set; }
        public ushort Id { get; set; }
        public ushort Variant { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public bool HasSignature
            => Id != 699 && Variant >> 12 == 5;
    }
}
