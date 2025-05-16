using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OmanClinicAppointmentSystem.Context;
using OmanClinicAppointmentSystem.Models;

namespace OmanClinicAppointmentSystem
{
    internal class Program
    {
        static List<Patient> patients = new();
        static List<Doctor> doctors = new();
        static List<Appointment> appointments = new();
        static void Main(string[] args)
        {
            int choice;

            do
            {
                Menu();
                Console.Write("Enter your choice: ");
                int.TryParse(Console.ReadLine(), out choice);

                switch (choice)
                {
                    case 1: RegisterPatient(); break;
                    case 2: AddDoctor(); break;
                    case 3: SearchDoctor(); break;
                    case 4: BookAppointment(); break;
                    case 5: ViewPatientAppointments(); break;
                    case 6: ViewAllAppointments(); break;
                    case 7: Console.WriteLine("\n👋 Thank you for using Oman Clinic Appointment System. Goodbye!"); break;
                    default: Console.WriteLine("Invalid choice. Try again."); break;
                }

                if (choice != 7)
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("Press any key to return to menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            } while (choice != 7);
        }

        static void Menu()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("Welcome to Oman Clinic Appointment System");
            Console.WriteLine("============================================\n");
            Console.WriteLine("1. Register New Patient");
            Console.WriteLine("2. Add New Doctor");
            Console.WriteLine("3. Search Doctor by Specialty");
            Console.WriteLine("4. Book Appointment");
            Console.WriteLine("5. View Patient Appointments");
            Console.WriteLine("6. View All Appointments");
            Console.WriteLine("7. Exit");
        }

        static void RegisterPatient()
        {
            using var context = new AppBdContext();
            Console.WriteLine("-- Register New Patient --");
            Console.Write("Enter Patient Name: ");
            var name = Console.ReadLine();
            Console.WriteLine("Enter National ID: ");
            var nationalId = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            var phone = Console.ReadLine();

            var patient = new Patient 
            { 
                Name = name,
                NationalId = nationalId,
                PhoneNumber = phone
            };
            context.Add(patient);
            context.SaveChanges();
            Console.WriteLine("Patient registered successfully!");
        }

        static void AddDoctor()
        {
            using var context = new AppBdContext();
            Console.WriteLine("-- Add New Doctor --");
            Console.Write("Enter Doctor Name: ");
            var name = Console.ReadLine();
            Console.WriteLine("Enter Specialty: ");
            var speciality = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            var phone = Console.ReadLine();

            var doctor = new Doctor
            {
                Name = name,
                Speciality = speciality,
                PhoneNumber = phone
            };
            context.Add(doctor);
            context.SaveChanges();
            Console.WriteLine("Doctor added successfully!");
        }


        static void SearchDoctor()
        {
            using var context = new AppBdContext();
            Console.WriteLine("-- Search Doctor by Specialty --");
            Console.Write("Enter Specialty to search: ");
            var speciality = Console.ReadLine();
            var matche = context.Doctors
                .Where(d => d.Speciality != null && d.Speciality.Equals(speciality, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matche.Any())
            {
                Console.WriteLine("\nDoctors Found:\n");
                matche.ForEach(d => Console.WriteLine($"- {d.Name} | Phone: {d.PhoneNumber}"));
            }
            else
            {
                Console.WriteLine("No doctors found with that specialty.");
            }
        }

        static void BookAppointment()
        {
            using var context = new AppBdContext();
            Console.WriteLine("-- Book Appointment --");
            Console.Write("Enter Patient National ID: ");
            var nationalId = Console.ReadLine();
            var patient = context.Patients.FirstOrDefault(p => p.NationalId == nationalId);
            if (patient == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            Console.Write("Enter Doctor Name: ");
            var Dname = Console.ReadLine();
            var doctor = context.Doctors.FirstOrDefault(d => d.Name == Dname);
            if (doctor == null)
            {
                Console.WriteLine("Doctor not found.");
                return;
            }

            Console.Write("Enter Appointment Date (dd/MM/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            var appointment = new Appointment {
                PatientId = patient.Id,
                DoctorId = doctor.Id, 
                AppontmentDate = date
            };
            context.Appointments.Add(appointment);
            context.SaveChanges();

            Console.WriteLine("Appointment booked successfully!");
        }


        static void ViewPatientAppointments()
        {
            using var context = new AppBdContext();
            Console.WriteLine("-- View Patient Appointments --");
            Console.Write("Enter Patient National ID: ");
            var nationalId = Console.ReadLine();
            var patient = context.Patients.FirstOrDefault(p => p.NationalId == nationalId);
            if (patient == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }
            var apps = context.Appointments
                .Include(a => a.doctor)
                .Where(a => a.PatientId == patient.Id)
                .ToList();

            Console.WriteLine($"\n📋 Appointments for {patient.Name}:\n");
            apps.ForEach(a => Console.WriteLine($"- Date: {a.AppontmentDate:dd/MM/yyyy} " +
                $"| Doctor: {a.doctor.Name} " +
                $"| Specialty: {a.doctor.Speciality}"));
        }


        static void ViewAllAppointments()
        {
            using var context = new AppBdContext();
            Console.WriteLine("-- View All Appointments --");
            Console.WriteLine("📅 All Booked Appointments:\n");

            var allApps = context.Appointments
                .Include(a => a.patient)
                .Include(a => a.doctor)
                .ToList();

            int i = 1;
            allApps.ForEach(a => Console.WriteLine(
                $"{i++}. Patient: {a.patient.Name} | Doctor: {a.doctor.Name} | Date: {a.AppontmentDate:dd/MM/yyyy} | Specialty: {a.doctor.Speciality}"));
        }
    }
}
