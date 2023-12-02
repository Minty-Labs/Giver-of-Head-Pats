namespace HeadPats.Configuration.Classes; 

public class RotatingStatus {
    public bool Enabled { get; set; }
    public List<Status> Statuses { get; set; }
}

public class Status {
    public int Id { get; set; }
    public string ActivityText { get; set; }
    public string ActivityType { get; set; }
    public string UserStatus { get; set; }
}