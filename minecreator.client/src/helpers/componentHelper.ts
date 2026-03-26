export function clickOutside(
  node: HTMLElement,
  onOutside?: () => void
): { update: (onOutside?: () => void) => void; destroy: () => void } {
  let callback = onOutside;

  const handleClick = (event: MouseEvent): void => {
    if (node && !node.contains(event.target as Node) && !event.defaultPrevented) {
      callback?.();
    }
  };

  document.addEventListener("click", handleClick, true);

  return {
    update(onOutside?: () => void) {
      callback = onOutside;
    },
    destroy() {
      document.removeEventListener("click", handleClick, true);
    },
  };
}