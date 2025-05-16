using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmanClinicAppointmentSystem.Models
{
    internal class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PNationalId { get; set; }
        public string DoctorName { get; set; }
        [Required]
        public DateTime AppontmentDate { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient patient { get; set; }
        public int  DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor doctor { get; set; }
    }
}
