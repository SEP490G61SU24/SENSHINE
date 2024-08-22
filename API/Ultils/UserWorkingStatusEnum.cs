namespace API.Ultils
{
    public enum UserWorkingStatusEnum
    {
        AVAILABLE, // Nhân viên đang sẵn sàng tiếp nhận khách hàng mới hoặc bắt đầu một dịch vụ.
        INSERVICE, // Nhân viên đang thực hiện một dịch vụ cho khách hàng.
        OFFDUTY, // Nhân viên đang không trong ca làm việc hoặc đã kết thúc ca làm.
        UNAVAILABLE // Nhân viên không thể làm việc do lý do cá nhân hoặc không thể tham gia ca làm.
    }
}
