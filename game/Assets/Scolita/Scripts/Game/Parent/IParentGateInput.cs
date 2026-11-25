namespace Scolita.Game.Parent
{
    /// <summary>Where the parent button and the gate's panel send the parent's touches; implemented by <see cref="ParentGateFlow"/>.</summary>
    public interface IParentGateInput
    {
        /// <summary>The parent button is pressed: the ring starts to fill. Called by the parent button.</summary>
        void Press();

        /// <summary>The parent button is let go: a ring not yet full empties. Called by the parent button.</summary>
        void Release();

        /// <summary>The number in place <paramref name="index"/> of the panel was tapped. Called by the gate's panel.</summary>
        void Choose(int index);

        /// <summary>The listen button: the number is said again. Called by the gate's panel.</summary>
        void Listen();

        /// <summary>The close button: the gate shuts without an answer. Called by the gate's panel.</summary>
        void Close();
    }
}
