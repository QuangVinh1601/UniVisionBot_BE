﻿namespace UniVisionBot.Configurations.DbConfig
{
    public class MyDatabase
    {
        public string ConnectionString { get; set; }
        public string DatabaseName {  get; set; }
        public string AppUsersCollectionName { get; set; }
        public string AppRolesCollectionName { get; set; }
        public string CareerPathCollectionName { get; set; }
        public string ChatLogsCollectionName { get; set; }
        public string ConsultantCollectionName { get; set; }
        public string ConsultationCollectionName { get; set; }
        public string FacultyCollectionName { get; set; }
        public string MajorCollectionName { get; set; }
        public string QuizCollectionName { get; set; }
        public string QuizResultCollectionName { get; set; }
        public string FeedbackCollectionName { get; set; }
        public string SuggestedUniversititesCollectionName { get; set; }
        public string UniversityCollectionName { get; set; }
        public string UserPreferenceCollectionName { get; set; }
        public string ConversationCollectionName { get; set; }
        public string MessageCollectionName { get; set; }
        public string ArticleCollectionName { get; set; }
        public string PendingConversationCollectionName { get; set; }

        public string RefreshTokenCollectionName { get; set; }
    }
}
