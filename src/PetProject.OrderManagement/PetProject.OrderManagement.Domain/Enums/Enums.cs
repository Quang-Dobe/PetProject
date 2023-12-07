namespace PetProject.OrderManagement.Domain.Enums
{
    public enum UserType
    {
        Sale = 0,                               // Nhân viên kinh doanh xuất nhập khẩu và Logistics
        DocumentStaff = 1,                      // Nhân viên chứng từ
        ProcurementOfficer = 2,                 // Nhân viên thu mua
        InternationalPaymentSpecialist = 3,     // Nhân viên thanh toán quốc tế
        OperationStaff = 4,                     // Nhân viên hiện trường
        Forwarder = 5,                          // Nhân viên giao/nhận vận tải
        WarehouseStaff = 6,                     // Nhân viên kho bãi cung ứng
        LogisticsCoodinator = 7,                // Nhân viên cảng
        CustomerService = 8,                    // Nhân viên chăm sóc khách hàng
        CustomsOfficer = 9,                     // Nhân viên khai báo hải quan
    }

    public enum CompanyType
    {
        Others = 0,
    }

    public enum OrganisationType
    {
        Sale = 0,                               // Sale, InternationalPaymentSpecialist
        Document = 1,                           // DocumentStaff
        Warehouse = 2,                          // WarehouseStaff, OperationStaff
        Logistic = 3,                           // LogisticsCoodinator
        Service = 4,                            // CustomerService, CustomsOfficer
        Transport = 5,                          // Forwarder
    }

    public enum StorageType
    {
        Bulk = 0,
        Liquid = 1,
        Others = 2,
    }

    public enum ContainerType
    {
        Bulk = 0,
        Liquid = 1,
        Others = 2,
    }

    public enum ContainerStatus
    {
        Ready = 0,
        InProcess = 1,
        OnSite = 2,
        Empty = 3,
        Full = 4,
    }

    public enum ProductType
    {
        Bulk = 0,
        Liquid = 1,
        Others = 2,
    }

    public enum ProductDangerousType
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }

    public enum DestinationType
    {
        Port = 0,
        Storage = 1,
        Others = 3,
    }

    public enum OrderStatus
    {
        Creat = 0,
        Ready = 1,
        Processing = 2,
        Delay = 3,
        Complete = 4,
        Others = 5,
    }
}
