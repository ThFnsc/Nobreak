namespace System
{
    public static class BoolExtensions
    {
        public static string SimNao(this bool input) =>
            input ? "Sim" : "Não";
    }
}
