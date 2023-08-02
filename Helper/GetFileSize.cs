namespace FlowSERVER1 {
    public class GetFileSize {
        public static double fileSize(byte[] getByte) {
            return getByte.Length / 1000000.0;
        }
    }
}
