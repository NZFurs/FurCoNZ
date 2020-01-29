﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using FurCoNZ.Web.Models;

namespace FurCoNZ.Web.ViewModels
{
    public class TicketDetailViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public int OrderId { get; }
        [Required]
        public TicketTypeViewModel TicketType { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Badge Name", Description = "This is the name displayed on your badge. This is typically your \"furry name\" or some other alias.")]
        public string BadgeName { get; set; }

        // Identification name is required for legal reasons, however this can be distressing in some circumstances.
        // We need to ensure this is never publically used if a preferred full name is supplied, and is only available to:
        // * FurcoNZ Staff, where necessary (such as registration)
        // * NZ officials, when legally required
        // * Where required for safety reasons (such as medical emergencies)
        [Required]
        [Display(Name = "Full Name (as it appears on your offical identification)")]
        public string IdentificationFullName { get; set; }

        // https://www.w3.org/International/questions/qa-personal-names
        [Display(Name = "Preferred Full Name")]
        public string PreferredFullName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth", Description = "Please enter this as \"yyyy-mm-dd\". That way, it's less ambiguous for our international visitors!")]
        [Range(typeof(DateTime), "1903-01-02","2999-12-30",ErrorMessage = "Please enter the ticket holder's date of birth", ParseLimitsInInvariantCulture = true)]
        public DateTime DateOfBirth { get; set; } = new DateTime(1900, 1, 1);

        // TODO: A real dietary requirements field

        [DataType(DataType.MultilineText)]
        [Display(Name = "Known Allergies / Medical Conditions")]
        public string KnownAllergies { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Cabin Preferences")]
        public string CabinPreferences { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Other Notes")]
        public string OtherNotes { get; set; }

        [Display(Name = "Terms and Conditions")]
        public TermsAndConditions TermsAndConditions { get; set; }

        [Display(Name = "Dietary Requirements")]
        public IEnumerable<FoodMenu> DietryRequirements { get; set; } = new List<FoodMenu>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationErrors = new List<ValidationResult>();

            if (TermsAndConditions == TermsAndConditions.NotAccepted)
            {
                validationErrors.Add(new ValidationResult("Ticket Holder must accept the Terms and Conditions before the order can be placed.", new List<string> { nameof(TermsAndConditions) }));
            }

            return validationErrors;
        }

        public TicketDetailViewModel()
        { }

        public TicketDetailViewModel(Ticket ticket)
        {
            Id = ticket.Id;

            OrderId = ticket.OrderId;

            // TODO: Ticket Type View Model?
            TicketType = new TicketTypeViewModel(ticket.TicketType);

            EmailAddress = ticket.EmailAddress;
            BadgeName = ticket.TicketName;
            PreferredFullName = ticket.PreferredName;
            IdentificationFullName = ticket.LegalName;

            DateOfBirth = ticket.DateOfBirth;

            var diet = new List<FoodMenu>();
            if (ticket.MealRequirements != Models.FoodMenu.Regular)
            {
                foreach (var value in Enum.GetValues(ticket.MealRequirements.GetType()))
                {
                    if (ticket.MealRequirements.HasFlag(value as Enum) && (int)value != 0)
                        diet.Add((FoodMenu)value);
                }
            }
            else
            {
                diet.Add(FoodMenu.Regular);
            }

            DietryRequirements = diet;
            KnownAllergies = ticket.KnownAllergens;
            CabinPreferences = ticket.CabinGrouping;

            OtherNotes = ticket.AdditionalNotes;

            TermsAndConditions = ticket.AcceptedTermsAndConditions 
                ? TermsAndConditions.Accepted 
                : TermsAndConditions.NotAccepted;

            CheckInTime = ticket.CheckInTime;
            CabinAssignment = ticket.CabinAssignment;
        }

        [Display(Name ="Checkin Time")]
        public DateTimeOffset? CheckInTime { get; set; }

        [Display(Name ="Is Checked In")]
        public bool IsCheckedIn { get => CheckInTime.HasValue; }

        [Display(Name ="Cabin Assignment")]
        public string CabinAssignment { get; set; }
    }

    public enum TermsAndConditions
    {
        [Display(Name = "I do not accept.")]
        NotAccepted = 0,
        [Display(Name = "I (the ticket holder) accept the terms and conditions.")]
        Accepted = 1,
        /// <summary>
        /// Allow the Terms to be agreed upon at a later time when the purchaser is unable to get the ticket holder to read the terms immediately.
        /// </summary>
        [Display(Name = "The ticket holder is unavailable right now to read and accept the terms and conditions.")]
        Deferred = 2,
    }

    /// <summary>
    /// Non [Flags] version of <see cref="Models.FoodMenu"/>
    /// </summary>
    public enum FoodMenu
    {
        Regular = 0,
        Vegetarian = 1,
        //Vegan = 2,
        DairyFree = 4,
        GlutenFree = 8
    }
}
