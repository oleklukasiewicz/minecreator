export class MenuItem {
  label: string;
  icon: string | null;
  value: string;
  type: string | null;
  constructor(
    label: string,
    value: string,
    icon: string | null = null,
    type: string | null = null
  ) {
    this.label = label;
    this.icon = icon;
    this.value = value;
    this.type = type;
  }
}
export class ToastItem {
  message: string;
  icon: string | null;
  duration: number;
  action: any;
  closeable: boolean;
  type: string | null;
  constructor(
    message: string,
    icon: string | null = null,
    type: "success" | "error" | "warning" | "info" | null = "success",
    action: any = () => {},
    closeable: boolean = true,
    duration: number = 3000
  ) {
    this.message = message;
    this.icon = icon;
    this.duration = duration;
    this.action = action;
    this.closeable = closeable;
    this.type = type;
  }
}
