using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace FurCoNZ.Web.Helpers
{
    internal static class FluentEmailExtensions
    {
        internal static FluentEmail.Core.Models.Address ToFluentEmailAddress(this MailAddress mailAddress)
        {
            return new FluentEmail.Core.Models.Address(mailAddress.Address, mailAddress.DisplayName);
        }

        internal static List<FluentEmail.Core.Models.Address> ToFluentEmailAddresses(this IEnumerable<MailAddress> mailAddresses)
        {
            return mailAddresses
                .Select(mailAddress => mailAddress.ToFluentEmailAddress())
                .ToList();
        }

        internal static IEnumerable<FluentEmail.Core.Models.Attachment> ToFluentEmailAttachments(this IEnumerable<System.Net.Mail.Attachment> attachments)
        {
            return attachments
                .Select(attachment => attachment.ToFluentEmailAttachment())
                .ToList();
        }

        internal static FluentEmail.Core.Models.Attachment ToFluentEmailAttachment(this System.Net.Mail.Attachment attachment)
        {
            return new FluentEmail.Core.Models.Attachment
            {
                ContentId = attachment.ContentId,
                ContentType = attachment.ContentType.MediaType,
                Data = attachment.ContentStream,
                Filename = attachment.ContentDisposition.FileName ?? Path.GetFileName((attachment.ContentStream as FileStream)?.Name),
                IsInline = attachment.ContentDisposition.Inline,
            };
        }
    }
}
