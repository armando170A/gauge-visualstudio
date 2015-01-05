﻿using System;
using System.Collections.Generic;
using System.Linq;
using main;

namespace Gauge.VisualStudio.Models
{
    public class Concept
    {
        private static IEnumerable<Concept> _allConcepts;
        public string StepValue { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }

        public static IEnumerable<Concept> GetAllConcepts()
        {
            var gaugeApiConnection = GaugeDTEProvider.GetApiConnectionForActiveDocument();
            var conceptsRequest = GetAllConceptsRequest.DefaultInstance;
            var apiMessage = APIMessage.CreateBuilder()
                .SetMessageId(GenerateMessageId())
                .SetMessageType(APIMessage.Types.APIMessageType.GetAllConceptsRequest)
                .SetAllConceptsRequest(conceptsRequest)
                .Build();

            var bytes = gaugeApiConnection.WriteAndReadApiMessage(apiMessage);

            return bytes.AllConceptsResponse.ConceptsList.Select(info => new Concept { StepValue = info.StepValue.ParameterizedStepValue, FilePath = info.Filepath, LineNumber = info.LineNumber });
        }

        private static long GenerateMessageId()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static Concept Search(string lineText)
        {
            _allConcepts = _allConcepts ?? GetAllConcepts();
            return _allConcepts.FirstOrDefault(concept => concept.StepValue == lineText);
        }

        public static void Refresh()
        {
            _allConcepts = GetAllConcepts();
        }
    }
}