using Microsoft.AspNetCore.Mvc;
using People.Models;
using System;

namespace People.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private static List<Person> _people = new List<Person>();

        [HttpGet]
        public IActionResult GetPeople()
        {
            return Ok(_people);
        }

        [HttpGet("{id}")]
        public IActionResult GetPerson(Guid id)
        {
            var person = _people.Find(p => p.Id == id);
            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [HttpPost]
        public IActionResult CreatePerson([FromBody] Person person)
        {
            person.Id = Guid.NewGuid();
            _people.Add(person);

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }


        [HttpDelete("{id}")]
        public IActionResult DeletePerson(Guid id)
        {
            var person = _people.Find(p => p.Id == id);
            if (person == null)
                return NotFound();

            _people.Remove(person);

            return NoContent();
        }

        [HttpPost("{id}/contact")]
        public IActionResult AddContact(Guid id, [FromBody] ContactInfo contact)
        {
            var person = _people.Find(p => p.Id == id);
            if (person == null)
                return NotFound();

            person.ContactInfos.Add(contact);

            return NoContent();
        }

        [HttpDelete("{id}/contact/{contactId}")]
        public IActionResult RemoveContact(Guid id, Guid contactId)
        {
            var person = _people.Find(p => p.Id == id);
            if (person == null)
                return NotFound();

            var contact = person.ContactInfos.Find(c => c.Id == contactId);
            if (contact == null)
                return NotFound();

            person.ContactInfos.Remove(contact);

            return NoContent();
        }
    }
}