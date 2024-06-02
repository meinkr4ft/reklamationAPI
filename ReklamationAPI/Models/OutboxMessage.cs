using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamationAPI.Models
{
    /// <summary>
    /// Outbox message model class for database interaction.
    /// </summary>
    public class OutboxMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Processed { get; set; }

        public OutboxMessage(DateTime date, string to, string subject, string body, bool processed)
        {
            Date = date;
            To = to;
            Subject = subject;
            Body = body;
            Processed = processed;
        }
        public OutboxMessage(Complaint complaint, string infoText)
        {
            this.Id = 0;
            this.Date = DateTime.Now;

            this.To = complaint.Customer.Email;
            this.Subject = $"Ihre Reklamation zu Produkt {complaint.ProductId}";
            this.Body = $"Guten Tag {complaint.Customer.Name},\n" +
                            $"\n" +
                            $"{infoText})\n" +
                            $"Informationen zu Ihrer Reklamation (Letzte Aktualisierung {this.Date}:\n" +
                            $"-Produkt ID: {complaint.Id}\n" +
                            $"-Email: {complaint.Customer.Email}\n" +
                            $"-Name: {complaint.Customer.Name}\n" +
                            $"-Eröffnungsdatum: {complaint.Date}\n" +
                            $"-Beschreibung: {complaint.Description}\n" +
                            $"-Status: {complaint.Status}\n" +
                            $"\n" +
                            $"Mit freundlichen Grüßen\n" +
                            $"ReklamationAPI\n";

            this.Processed = false;
        }
    }
}
