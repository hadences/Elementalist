public class Element {
    
    public enum Type {
        FIRE,
        WATER,
        GRASS
    }

    public static Type getWeakness(Type element){
        switch(element){
            case Type.FIRE:
                return Type.WATER;
            case Type.WATER:
                return Type.GRASS;
            case Type.GRASS:
                return Type.FIRE;
            default:
                return Type.FIRE;
        }
    }

    public static Type getStrength(Type element){
        switch(element){
            case Type.FIRE:
                return Type.GRASS;
            case Type.WATER:
                return Type.FIRE;
            case Type.GRASS:
                return Type.WATER;
            default:
                return Type.FIRE;
        }
    }
}