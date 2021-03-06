using System;
using System.Linq;
using System.Threading.Tasks;
using EasyEventSourcing;
using EasyEventSourcing.Aggregate;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quiz.Domain;
using Quiz.Domain.Commands;

namespace Quiz.Api
{
    [Route("[controller]")]
    public class QuizController
    {
        private readonly QuizAppService _quizAppService;
        private readonly IEventStoreProjections _projectionsClient;

        public QuizController(
            QuizAppService quizAppService, 
            IEventStoreProjections projectionsClient)
        {
            _quizAppService = quizAppService;
            _projectionsClient = projectionsClient;
        }

        [HttpGet]
        public async Task<QuizReadModel> Get()
        {
            var result = await _projectionsClient.GetStateAsync(nameof(Projections.QuestionAnswers)); 
            return JsonConvert.DeserializeObject<QuizReadModel>(result);
        }

        [HttpPost]
        [Route("{id}")]
        public async Task Vote(Guid id, [FromBody]QuizAnswersCommand quizAnswersComand) =>
            await _quizAppService.Vote(new QuizAnswersCommand(id, quizAnswersComand.Answers));

        [HttpPut]
        public async Task<object> Start([FromBody]QuizModel quizModel) => 
            await _quizAppService.Start(quizModel);

        [HttpDelete]
        [Route("{id}")]
        public async Task Close(Guid id) =>
            await _quizAppService.Close(id);
    }
}
