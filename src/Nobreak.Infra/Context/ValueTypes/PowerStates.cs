using System.ComponentModel.DataAnnotations;

namespace Nobreak.Context.Entities
{
    public enum PowerStates
    {
        [Display(Name = "Rede")]
        Grid,

        [Display(Name = "Bateria")]
        Battery
    }
}
