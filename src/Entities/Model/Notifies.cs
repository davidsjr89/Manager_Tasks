using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Model
{
    public class Notifies
    {
        public Notifies()
        {
            Notitycoes = new List<Notifies>();
        }

        [NotMapped]
        public string NameProperty { get; set; }

        [NotMapped]
        public string Message { get; set; }

        [NotMapped]
        public List<Notifies> Notitycoes { get; set; }

        public bool IsPropertyString(string value, string nameProperty)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(nameProperty))
            {
                Notitycoes.Add(new Notifies
                {
                    Message = "Campo obrigatório",
                    NameProperty = nameProperty
                });
                return false;
            }

            return true;
        }

        public bool IsPropertyInt(int value, string nameProperty)
        {
            if (value < 1 || string.IsNullOrWhiteSpace(nameProperty))
            {
                Notitycoes.Add(new Notifies
                {
                    Message = "Campo obrigatório",
                    NameProperty = nameProperty
                });
                return false;
            }

            return true;
        }
    }
}
