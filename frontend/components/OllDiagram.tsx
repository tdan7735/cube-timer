import { ollLayouts } from "../lib/ollLayouts";

export function OllDiagram({ caseNumber }: { caseNumber: number }) {
  const layout = ollLayouts[caseNumber];
  if (!layout) return null;
  const [top, back, front, left, right] = layout;
  const strips = [
    { stickers: [...back].reverse(), x: 30, y: 10, vertical: false },
    { stickers: [...front], x: 30, y: 124, vertical: false },
    { stickers: [...left], x: 10, y: 30, vertical: true },
    { stickers: [...right].reverse(), x: 124, y: 30, vertical: true },
  ];
  const color = (sticker: string) => sticker === "y" ? "#facc15" : "#555";

  return (
    <svg className="h-[170px] w-[154px] shrink-0 self-center" viewBox="0 0 154 170" role="img"
      aria-label={`OLL ${caseNumber}: top view showing yellow sticker orientation`}>
      <title>OLL {caseNumber} case</title>
      <desc>Yellow stickers show the last-layer orientation; grey stickers are other colours.
        Front is at the bottom. Alternatives may use a different starting grip.</desc>
      <g stroke="#111" strokeWidth="2">
        {[...top].map((sticker, i) => (
          <rect key={i} x={30 + (i % 3) * 30} y={30 + Math.floor(i / 3) * 30}
            width="30" height="30" rx="2" fill={color(sticker)} />
        ))}
        {strips.map((strip, side) => strip.stickers.map((sticker, i) => (
          <rect key={`${side}-${i}`} x={strip.x + (strip.vertical ? 0 : i * 30)}
            y={strip.y + (strip.vertical ? i * 30 : 0)}
            width={strip.vertical ? 16 : 30} height={strip.vertical ? 30 : 16}
            rx="2" fill={color(sticker)} />
        )))}
      </g>
      <text x="75" y="160" textAnchor="middle" fill="#aaa" fontSize="10">FRONT</text>
    </svg>
  );
}
