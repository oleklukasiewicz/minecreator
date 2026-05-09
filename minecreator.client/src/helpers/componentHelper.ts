export function clickOutside(
  node: HTMLElement,
  onOutside?: (event?: MouseEvent) => void
): {
  update: (onOutside?: (event?: MouseEvent) => void) => void;
  destroy: () => void;
} {
  let callback = onOutside;

  const handleClick = (event: MouseEvent): void => {
    if (node && !node.contains(event.target as Node) && !event.defaultPrevented) {
      callback?.(event);
    }
  };

  document.addEventListener("click", handleClick, true);

  return {
    update(onOutside?: (event?: MouseEvent) => void) {
      callback = onOutside;
    },
    destroy() {
      document.removeEventListener("click", handleClick, true);
    },
  };
}