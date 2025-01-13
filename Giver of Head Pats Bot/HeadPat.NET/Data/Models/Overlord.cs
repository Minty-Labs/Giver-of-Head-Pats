using System.ComponentModel.DataAnnotations;

namespace HeadPats.Data.Models; 

public class Overlord {
    [Key] public ulong ApplicationId { get; set; }
    public int PatCount { get; set; }
    public int NsfwCommandsUsed { get; set; }
}

// public static class OverlordControl {
//     public static void AddToCommandCounter() {
//         using var db = new Context();
//         var checkOverall = db.Overall.AsQueryable()
//             .Where(u => u.ApplicationId.Equals(Vars.ClientId)).ToList().FirstOrDefault();
//         
//         if (checkOverall == null) {
//             var overall = new Overlord {
//                 ApplicationId = Vars.ClientId,
//                 PatCount = 0,
//                 NsfwCommandsUsed = 0
//             };
//             db.Overall.Add(overall);
//             db.SaveChanges();
//         }
//         else {
//             checkOverall.NsfwCommandsUsed += 1;
//             db.Overall.Update(checkOverall);
//         }
//         
//         db.SaveChanges();
//     }
// }