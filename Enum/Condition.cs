using System.ComponentModel.DataAnnotations;

public enum Condition
{
    [Display(Name ="Brand New")]
    Brand_New,
    [Display(Name ="Like New")]
    Like_New,
    [Display(Name ="Used")]
    Used,
    [Display(Name ="Not Working")]
    Not_Working
}