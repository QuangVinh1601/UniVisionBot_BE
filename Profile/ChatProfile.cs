﻿using AutoMapper;
using UniVisionBot.DTOs.Chat;
using UniVisionBot.Models;

namespace UniVisionBot.Profiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Message, MessageResponse>();
            CreateMap<Conversation, ConversationResponse>();
            CreateMap<AppUser, UserResponse>();
        }
    }
}