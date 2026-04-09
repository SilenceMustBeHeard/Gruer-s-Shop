using GruersShop.Data.Models.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Configuration.Message;

public class ContactMessageConfig : BaseMessageConfig<ContactMessage>
{
    public override void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedContactMessages)
            .HasForeignKey(m => m.ReceiverId);

        builder
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentContactMessages)
            .HasForeignKey(m => m.SenderId);

        // RespondedBy relationship
        builder
            .HasOne(m => m.RespondedBy)
            .WithMany()
            .HasForeignKey(m => m.RespondedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}