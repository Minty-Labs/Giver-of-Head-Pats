namespace HeadPats.Configuration.Classes; 

public class RotatingStatus {
    public bool Enabled { get; set; }
    public List<Status> Statuses { get; init; }
}

public class Status {
    public int Id { get; init; }
    public string ActivityText { get; set; }
    public string ActivityType { get; set; }
    public string UserStatus { get; set; }
}