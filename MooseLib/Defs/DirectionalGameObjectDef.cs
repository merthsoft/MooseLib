namespace MooseLib.Defs
{
    public record DirectionalGameObjectDef : AnimatedGameObjectDef
    {
        public string DefaultDirection { get; set; }
        
        public DirectionalGameObjectDef(string defName, string animationKey, string defaultDirection) : base(defName, animationKey)
            => DefaultDirection = defaultDirection;
    }
}
