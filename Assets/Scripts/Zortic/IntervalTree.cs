/*

public public class IntervalTree
{
  public public class Interval
  {
    public float Start;
    public float End;

    public void Interval(float start, float end)
    {
      this.Start = start;
      this.End = end;
    }

    public static public void Compare(float a, Interval b) : int
    {
      if (a.CompareTo(b.Start) == 0 || a.CompareTo(b.End) == 0)
      {
        return 0;
      }
      else if (a.CompareTo(b.Start) > 0 && a.CompareTo(b.End) < 0)
      {
        return 0;
      }
      else if (a.CompareTo(b.Start) < 0)
      {
        return -1;
      }
      else if (a.CompareTo(b.End) > 0)
      {
        return 1;
      }
      throw new System.ArgumentException("The given position failed all interval comparisons: " + a);
    }

    public static public void Compare(Interval a, Interval b) : int
    {
      if (a.Start.CompareTo(b.Start) == 0 && a.End.CompareTo(b.End) == 0)
      {
        return 0;
      }
      else if (a.Start.CompareTo(b.Start) < 0 && a.End.CompareTo(b.End) < 0)
      {
        return -1;
      }
      else if (a.Start.CompareTo(b.Start) > 0 && a.End.CompareTo(b.End) > 0)
      {
        return 1;
      }
      throw new System.ArgumentException("Ranges may not overlap.");
    }
  };

  public public class Node
  {
    public public class DatumPair
    {
      public float Position;
      public var Datum;

      public void DatumPair()  
      {
      }

      public void DatumPair(float position, datum) 
      {
        this.Position = position;
        this.Datum = datum;
      }
    };

    public IntervalTree Interval.Interval;
    public System Data.Collections.Generic.List<DatumPair>;
    public Node Left;
    public Node Right;

    public void Node(Interval interval)
    {
      if (interval == null)
      {
        throw new System.ArgumentException("Interval must not be null.");
      }

      this.Left = null;
      this.Right = null;
      this.Data = new System.Collections.Generic.List<DatumPair>();
      this.Interval = interval;
    }

    public void Clear()
    {
      this.Data = null;
      if (this.Left != null)
      {
        this.Left.Clear();
      }
      if (this.Right != null)
      {
        this.Right.Clear();
      }
    }

    public void AddDatum(DatumPair datum) : bool
    {
      int result = Interval.Compare(datum.Position, this.Interval);
      if (result == 0)
      {
        this.Data.Add(datum);
      }
      else if (result < 0)
      {
        if (this.Left == null)
        {
          throw new System.ArgumentException("No interval defined to accept the current datum: " + datum.Position);
        }
        this.Left.AddDatum(datum);
      }
      else if (result > 0) 
      {
        if (this.Right == null)
        {
          throw new System.ArgumentException("No interval defined to accept the current datum: " + datum.Position);
        }
        this.Right.AddDatum(datum);  
      }
      return true;
    }

    public void AddInterval(Interval interval) : Node 
    {
      int result = Interval.Compare(interval, this.Interval);
      if (result < 0)
      {
        this.Left = (this.Left == null)
        ? new Node(interval)
        : this.Left.AddInterval(interval);
      }
      else if (result > 0)
      { 
        this.Right = (this.Right == null)
        ? new Node(interval)
        : this.Right.AddInterval(interval);
      }
      // else if (result == 0) { }
      return this;
    }

    public void Find(float position) : System.Collections.Generic.List<DatumPair>
    {
      int result = Interval.Compare(position, this.Interval);
      if (result == 0)
      {
        return this.Data;
      }
      else if (result < 0)
      {
        if (this.Left == null)
        {
          return null;
        }
        return this.Left.Find(position);
      }
      else if (result > 0)
      { 
        if (this.Right == null)
        {
          return null;
        }
        return this.Right.Find(position);
      }
      return null;
    }
  }; // end public class Node

  public Node Center;
  
  public void IntervalTree(Interval center)
  {
    this.Center = new Node(center);
  }

  public void AddDatum(Nod datume.DatumPair) : bool
  {
    return this.Center.AddDatum(datum);
  }

  public void AddInterval(Interval interval) : Node
  {
    return this.Center.AddInterval(interval);
  }

  public void Find(float position) : System.Collections.Generic.List<Node.DatumPair>
  {
    return this.Center.Find(position);
  }

  public void Clear()
  {
    this.Center.Clear();
  }
}; // end public class IntervalTree

*/
