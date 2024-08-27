namespace API.Ultils
{
    public enum WorkscheduleStatusEnum
    {
        AVAILABLE,       // Sẵn sàng = > sẵn sàng chờ việc
        SCHEDULED,       // Có khách đặt lịch
        COMPLETED,       // Đã làm xong
        CANCELED,        // Đã hủy do một lý do nào đó hoặc không làm (Không được chấm công)
        INPROGRESS       // Đang làm
    }
}
