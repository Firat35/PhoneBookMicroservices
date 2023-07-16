using Microsoft.AspNetCore.Mvc;
using Moq;
using People.Controllers;
using People.Models;
using People.Repository;

namespace Test
{
    public class PeopleControllerTest
    {
        private readonly Mock<IRepository<Person>> _mockPersonRepo;
        private readonly Mock<IRepository<ContactInfo>> _mockContactRepo;
        private readonly PeopleController _controller;
        private List<Person> people = new List<Person>();
        private List<ContactInfo> contacts = new List<ContactInfo>();

        public PeopleControllerTest()
        {
            _mockPersonRepo = new Mock<IRepository<Person>>();
            _mockContactRepo = new Mock<IRepository<ContactInfo>>();
            _controller = new PeopleController(_mockPersonRepo.Object, _mockContactRepo.Object);

            people = new List<Person>() { 
            new Person { Id = new Guid("1b802ff8-48b5-4356-8732-287231b80cf4"), FirstName = "ahmet", LastName = "palta", Company = "gunkom"},
            new  Person { Id = new Guid("b5dc2938-7234-4820-a05b-dac4f09e3770"), FirstName = "mehmet", LastName = "test", Company = "gunkom2"}};


            contacts = new List<ContactInfo>() {
                new ContactInfo {
                    Id = new Guid("929a288c-4346-4be4-8dd2-2dce7b332aaf"),
                    InfoType = "location",
                    InfoContent = "withco",
                    PersonId = new Guid("1b802ff8-48b5-4356-8732-287231b80cf4")} ,
                new ContactInfo {
                    Id = new Guid("866eb0e9-4460-4d0c-96d5-45e568a09954"),
                    InfoType = "number",
                    InfoContent = "456674" ,
                    PersonId = new Guid("b5dc2938-7234-4820-a05b-dac4f09e3770")} };
        }

        [Fact]
        public async void GetPerson_ActionExecutes_ReturnOkResultWithPerson()
        {
            _mockPersonRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(people);

            var result = await _controller.GetPeople();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnPeople = Assert.IsAssignableFrom<IEnumerable<PersonDto>>(okResult.Value);

            Assert.Equal<int>(2, returnPeople.ToList().Count);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async void GetPerson_IdInValid_ReturnNotFound(string personId)
        {
            Person person = null;

            _mockPersonRepo.Setup(x => x.GetByIdAsync(new Guid(personId))).ReturnsAsync(person);

            var result = await _controller.GetPerson(new Guid(personId));

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("1b802ff8-48b5-4356-8732-287231b80cf4")]
        [InlineData("b5dc2938-7234-4820-a05b-dac4f09e3770")]
        public async void GetPerson_IdValid_ReturnOkResult(string personId)
        {
            var person = people.First(x => x.Id == new Guid(personId));
            //var contactList = contacts.Where(x => x.PersonId == new Guid(personId)).ToList();

            _mockPersonRepo.Setup(x => x.GetByIdAsync(new Guid(personId))).ReturnsAsync(person);
            //_mockContactRepo.Setup(x => x.GetListAsync(y => y.PersonId == new Guid(personId))).ReturnsAsync(contactList);

            var result = await _controller.GetPerson(new Guid(personId));

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnPerson = Assert.IsType<Person>(okResult.Value);

            Assert.Equal(new Guid(personId), returnPerson.Id);
            Assert.Equal(person.FirstName, returnPerson.FirstName);
        }

        [Fact]
        public async void CreatePerson_ActionExecutes_ReturnCreatedAtAction()
        {
            var person = people.First();

            var personDetailDto = new PersonDetailDto { FirstName =person.FirstName, LastName= person.LastName, Company = person.Company };

            var result = await _controller.CreatePerson(personDetailDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal("GetPerson", createdAtActionResult.ActionName);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async void DeletePerson_IdInValid_ReturnNotFound(string personId)
        {
            Person person = null;

            //_mockPersonRepo.Setup(x => x.GetByIdAsync(new Guid(personId))).ReturnsAsync(person);

            var resultNotFound = await _controller.DeletePerson(new Guid(personId));

            Assert.IsType<NotFoundResult>(resultNotFound);
        }

        [Theory]
        [InlineData("b5dc2938-7234-4820-a05b-dac4f09e3770")]
        public async void DeletePerson_ActionExecute_ReturnNoContent(string personId)
        {
            var person = people.First(x => x.Id ==  new Guid(personId));
            _mockPersonRepo.Setup(x => x.GetByIdAsync(new Guid(personId))).ReturnsAsync(person);
            _mockPersonRepo.Setup(x => x.DeleteAsync(person));

            var noContentResult = await _controller.DeletePerson(new Guid(personId));

            _mockPersonRepo.Verify(x => x.DeleteAsync(person), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult);
        }
    }
}