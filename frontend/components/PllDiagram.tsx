// Top-layer side stickers from https://speedcubedb.com/a/3x3/PLL.
// Each face is recorded left-to-right when looking directly at that face.
// Diagrams show a reference orientation; alternate algorithms may use another grip.
const layouts: Record<string, readonly [string, string, string, string]> = {
  Aa: ["obo", "ggb", "brr", "rog"],
  Ab: ["rbg", "ggo", "orr", "bob"],
  E: ["bog", "grb", "obr", "rgo"],
  F: ["bog", "rrr", "ogb", "gbo"],
  Ga: ["ogb", "goo", "rbr", "brg"],
  Gb: ["orb", "gbo", "ror", "bgg"],
  Gc: ["oob", "gbo", "rgr", "brg"],
  Gd: ["ogb", "gro", "ror", "bbg"],
  H: ["bgb", "gbg", "ror", "oro"],
  Ja: ["ggo", "oob", "bbg", "rrr"],
  Jb: ["goo", "rgg", "bbb", "orr"],
  Na: ["gbb", "bgg", "roo", "orr"],
  Nb: ["bbg", "ggb", "oor", "rro"],
  Ra: ["ogr", "bob", "gbo", "rrg"],
  Rb: ["bog", "rgr", "obb", "gro"],
  T: ["obb", "ggo", "ror", "brg"],
  Ua: ["brb", "ggg", "ror", "obo"],
  Ub: ["bob", "ggg", "rbr", "oro"],
  V: ["bog", "ggb", "orr", "rbo"],
  Y: ["brg", "ggb", "obr", "roo"],
  Z: ["gog", "brb", "ogo", "rbr"],
};

const colors: Record<string, string> = {
  r: "#ef4444", o: "#fb923c", b: "#3b82f6", g: "#22c55e",
};

export function PllDiagram({ name }: { name: string }) {
  const layout = layouts[name];
  if (!layout) return null;
  const [back, front, left, right] = layout;
  const strips = [
    { stickers: [...back].reverse(), x: 30, y: 10, vertical: false },
    { stickers: [...front], x: 30, y: 124, vertical: false },
    { stickers: [...left], x: 10, y: 30, vertical: true },
    { stickers: [...right].reverse(), x: 124, y: 30, vertical: true },
  ];

  return (
    <svg className="case-diagram" viewBox="0 0 154 170" role="img"
      aria-label={`${name} permutation: top view with yellow face and side sticker pattern`}>
      <title>{name} PLL case</title>
      <desc>Reference orientation, front at the bottom. Alternatives may use a different starting grip.</desc>
      <g stroke="#111" strokeWidth="2">
        {Array.from({ length: 9 }, (_, i) => (
          <rect key={i} x={30 + (i % 3) * 30} y={30 + Math.floor(i / 3) * 30}
            width="30" height="30" rx="2" fill="#facc15" />
        ))}
        {strips.map((strip, side) => strip.stickers.map((color, i) => (
          <rect key={`${side}-${i}`} x={strip.x + (strip.vertical ? 0 : i * 30)}
            y={strip.y + (strip.vertical ? i * 30 : 0)}
            width={strip.vertical ? 16 : 30} height={strip.vertical ? 30 : 16}
            rx="2" fill={colors[color]} />
        )))}
      </g>
      <text x="75" y="160" textAnchor="middle" fill="#aaa" fontSize="10">FRONT</text>
    </svg>
  );
}
